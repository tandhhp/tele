import { PageContainer, ProForm, ProFormSelect } from '@ant-design/pro-components';
import { Col, Row } from 'antd';
import SaleChart from './components/sale-chart';
import TopSales from './components/top-month';
import SmChart from './components/sm-chart';
import { apiBranchOptions } from '@/services/settings/branch';
import ContactStatistics from './components/contact';

const HomePage: React.FC = () => {

  return (
    <PageContainer extra={(
      <ProForm submitter={false}>
        <ProFormSelect request={apiBranchOptions} className="w-32" name={`branchId`} formItemProps={{
          className: 'mb-0'
        }} initialValue={1} />
      </ProForm>
    )}>
      <div className='grid grid-cols-1 md:grid-cols-2 gap-4 mb-4'>
        <ContactStatistics />
        <ContactStatistics />
      </div>
      <Row gutter={16}>
        <Col xs={24} md={18}>
          <Row gutter={16}>
            <Col md={12} xs={24}>
              <SaleChart />
            </Col>
            <Col md={12} xs={24}>
              <SmChart />
            </Col>
          </Row>
        </Col>
        <Col xs={24} md={6}>
          <TopSales />
        </Col>
      </Row>
    </PageContainer>
  );
};

export default HomePage;
