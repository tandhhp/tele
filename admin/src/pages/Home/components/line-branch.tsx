import { apiLineChartMonth } from "@/services/order";
import { Line } from "@ant-design/charts";
import { ProCard } from "@ant-design/pro-components";
import { useAccess } from "@umijs/max";
import { Col, Select } from "antd"
import dayjs, { Dayjs } from "dayjs";
import { useEffect, useState } from "react";

type Props = {
    branch: number;
}

const LineBranch : React.FC<Props> = ({ branch }) => {

    const access = useAccess();
    const [data, setData] = useState<any>();
    const [year, setYear] = useState<Dayjs | null>(dayjs());

    useEffect(() => {
        if (year) {
            apiLineChartMonth({
                year: year.year(),
                branch
            }).then(response => setData(response));
        }
    }, [branch])

    return (
        <Col xs={24} md={24} hidden={!access.canViewChart}>
            <ProCard title={`Doanh thu năm ${dayjs().year()}`} headerBordered className="mb-4">
                <Line
                    height={400}
                    className="h-full"
                    data={data}
                    xField='month'
                    yField="amount"
                    axis={{
                        y: {
                            labelFormatter: (v: any) => `${v}`.replace(/\d{1,3}(?=(\d{3})+$)/g, (s) => `${s},`)
                        }
                    }}
                />
            </ProCard>
        </Col>
    )
}

export default LineBranch;