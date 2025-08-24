import { apiAddLead, apiUpdateLead } from "@/services/contact";
import { apiSalesWithSmOptions, apiTeleWithTmOptions } from "@/services/user";
import { BRANCH_OPTIONS, LeadStatus } from "@/utils/constants";
import { MinusCircleOutlined, PlusOutlined } from "@ant-design/icons";
import { ModalForm, ModalFormProps, ProFormDatePicker, ProFormInstance, ProFormSelect, ProFormText, ProFormTextArea } from "@ant-design/pro-components"
import { Button, Col, Divider, Form, Input, message, Row } from "antd";
import { Fragment, useEffect, useRef } from "react";
import { useAccess } from "@umijs/max";
import dayjs from "dayjs";

type Props = ModalFormProps & {
    reload: any;
    lead?: any;
    eventDate: string;
}

const LeadForm: React.FC<Props> = (props) => {
    const access = useAccess();
    const eventTime = window.location.href.includes('am') ? '09:00' : '14:30';
    const formRef = useRef<ProFormInstance>();
    const { lead } = props;

    useEffect(() => {
        if (lead) {
            formRef.current?.setFields([
                {
                    name: 'name',
                    value: lead.name
                },
                {
                    name: 'phoneNumber',
                    value: lead.phoneNumber
                },
                {
                    name: 'dateOfBirth',
                    value: lead.dateOfBirth
                },
                {
                    name: 'gender',
                    value: lead.gender
                },
                {
                    name: 'jobTitle',
                    value: lead.jobTitle
                },
                {
                    name: 'salesId',
                    value: lead.salesId
                },
                {
                    name: 'identityNumber',
                    value: lead.identityNumber
                },
                {
                    name: 'address',
                    value: lead.address
                },
                {
                    name: 'note',
                    value: lead.note
                },
                {
                    name: 'id',
                    value: lead.id
                },
                {
                    name: 'branch',
                    value: lead.branch
                },
                {
                    name: 'telesaleId',
                    value: lead.telesaleId
                }
            ])
        }
    }, [props.lead]);

    return (
        <ModalForm {...props} title="Khách hàng tiềm năng tại sự kiện" onFinish={async (values) => {
            values.eventTime = eventTime;
            if (values.dateOfBirth) {
                values.dateOfBirth = dayjs(values.dateOfBirth).format('YYYY-MM-DD');
            }
            values.status = LeadStatus.Checkin;
            if (values.id) {
                values.eventDate = props.lead?.eventDate;
                await apiUpdateLead(values);
            } else {
                values.eventDate = props.eventDate;
                await apiAddLead(values);
            }
            message.success('Thành công!');
            props?.reload();
            props.onOpenChange?.(false);
        }} formRef={formRef}>
            <ProFormText name="id" hidden />
            <Row gutter={16}>
                <Col md={12} xs={12}>
                    <ProFormText label="Họ và tên" name="name" rules={[
                        {
                            required: true
                        }
                    ]} />
                </Col>
                <Col md={6} xs={12}>
                    <ProFormText label="Số điện thoại" name="phoneNumber" rules={[
                        {
                            required: true
                        }
                    ]} />
                </Col>
                <Col md={6} xs={12}>
                    <ProFormDatePicker label="Ngày sinh" name="dateOfBirth" width="xl" fieldProps={{
                        format: {
                            type: 'mask',
                            format: 'DD-MM-YYYY'
                        }
                    }} />
                </Col>
                <Col md={4} xs={12}>
                    <ProFormSelect label="Giới tính" name="gender" options={[
                        {
                            label: 'Nam',
                            value: true as any
                        },
                        {
                            label: 'Nữ',
                            value: false as any
                        },
                        {
                            label: 'Khác',
                            value: null
                        }
                    ]} />
                </Col>
                <Col md={6} xs={12}>
                    <ProFormSelect request={apiSalesWithSmOptions} fieldProps={{
                        popupMatchSelectWidth: false
                    }} name="salesId" label="Trợ lý cá nhân" disabled={access.sales || access.telesale || access.telesaleManager} showSearch />
                </Col>
                <Col md={6} xs={12}>
                    <ProFormSelect request={apiTeleWithTmOptions} fieldProps={{
                        popupMatchSelectWidth: false
                    }} name="telesaleId" label="Người gọi" disabled={access.sales || access.telesale || access.telesaleManager} showSearch />
                </Col>
                <Col md={8} xs={12}>
                    <ProFormText name="identityNumber" label="CCCD" />
                </Col>
                <Col xs={12} md={4}>
                    <ProFormSelect rules={[
                        {
                            required: true
                        }
                    ]} options={BRANCH_OPTIONS} name="branch" label="Chi nhánh" />
                </Col>
                <Col md={20} xs={24}>
                    <ProFormText name="address" label="Địa chỉ" />
                </Col>
                <Col md={24} xs={24}>
                    <ProFormTextArea name="note" label="Ghi chú" />
                </Col>
                {
                    !props.lead && (
                        <>
                            <Divider>Người đi cùng</Divider>
                            <Form.List name="subLeads">
                                {(fields, { add, remove }) => (
                                    <div className="flex flex-col w-full">
                                        {fields.map(({ key, name, ...restField }) => (
                                            <div key={key} className="flex-1">
                                                <Row gutter={16} className="w-full">
                                                    <Col md={12} xs={12}>
                                                        <Form.Item {...restField} name={[name, 'name']} label="Họ và tên">
                                                            <Input />
                                                        </Form.Item>
                                                    </Col>
                                                    <Col md={6} xs={12}>
                                                        <Form.Item {...restField} name={[name, 'phoneNumber']} label="Số điện thoại">
                                                            <Input />
                                                        </Form.Item>
                                                    </Col>
                                                    <Col md={6} xs={12}>
                                                        <ProFormSelect label="Giới tính" {...restField} name={[name, 'gender']} options={[
                                                            {
                                                                label: 'Nam',
                                                                value: true as any
                                                            },
                                                            {
                                                                label: 'Nữ',
                                                                value: false as any
                                                            },
                                                            {
                                                                label: 'Khác',
                                                                value: null
                                                            }
                                                        ]} />
                                                    </Col>
                                                    <Col span={2}>
                                                        <Form.Item label={<Fragment />}>
                                                            <MinusCircleOutlined onClick={() => {
                                                                remove(name);
                                                            }} />
                                                        </Form.Item>
                                                    </Col>
                                                </Row>
                                            </div>
                                        ))}
                                        <div>
                                            <Form.Item>
                                                <Button type="dashed" onClick={() => add()} block icon={<PlusOutlined />}>
                                                    Thêm người đi cùng
                                                </Button>
                                            </Form.Item>
                                        </div>
                                    </div>
                                )}
                            </Form.List>
                        </>
                    )
                }
            </Row>

        </ModalForm>
    )
}

export default LeadForm;